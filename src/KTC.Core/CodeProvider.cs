namespace KTL.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Octokit;

    public sealed class CodeProvider
    {
        private const int LinesCount = 10;
        private readonly Random _random = new Random();
        private readonly HttpClient _httpClient = new HttpClient();
        private readonly GitHubClient _github = new GitHubClient(new ProductHeaderValue("Know-This-Code"));
        private readonly IReadOnlyDictionary<Language, string> _languages = new Dictionary<Language, string>
        {
            { Language.C, "c" },
            { Language.CSharp , "cs"},
            { Language.Java, "java" },
            { Language.JavaScript, "js" },
            { Language.FSharp, "fs" },
            { Language.Go, "go" },
            { Language.Scala, "scala" },
            { Language.Haskell, "hs" },
            { Language.Php, "php" },
            { Language.VisualBasic, "vb" },
            { Language.Rust, "rs" },
            { Language.Lua, "lua" },
            { Language.CPlusPlus, "cpp" },
            { Language.Perl, "pl" },
            { Language.Python, "py" },
            { Language.R, "r" } 
        };

        public IEnumerable<Language> RandomLanguages => _languages.Keys.OrderBy(_ => _random.Next());

        public Language GetRandomLanguage ()
        {
            return _languages.Keys.GetRandomElement();
        }

        public async Task<string> GetRandomCodeSample (Language language)
        {
            var repository = await GetRandomRepository(language);
            var code = await GetRandomCode(repository, language);
            return code;
        }

        private async Task<Repository> GetRandomRepository(Language language)
        {
            var searchRepositoryRequest = new SearchRepositoriesRequest 
            { 
                Language = language,  
                Stars = Range.GreaterThan(100)
            };
            var searchRepositoryResult = await _github.Search.SearchRepo(searchRepositoryRequest);
            var repository = searchRepositoryResult.Items.GetRandomElement(_random);
            return repository;
        }

        private async Task<string> GetRandomCode(Repository repository, Language language)
        {
            var searchCodeRequest = new SearchCodeRequest 
            { 
                Repos = new RepositoryCollection { repository.FullName },
                Language = language,
                Extension = _languages[language],
                Size = Range.GreaterThan(5)
            };
            var searchCodeResult = await _github.Search.SearchCode(searchCodeRequest);
            var searchCode = searchCodeResult.Items.GetRandomElement(_random);
            var fullCode = await DownloadCodeSample(searchCode.HtmlUrl);
            var codeSample = GetCodeSample(fullCode);
            return codeSample;
        }

        private async Task<string> DownloadCodeSample(string githubUrl)
        {
            var url = GetRawGitHubUrl(githubUrl);
            var code = await _httpClient.GetAsync(url);
            return await code.Content.ReadAsStringAsync();;
        }

        private string GetRawGitHubUrl(string githubUrl)
        {
            return githubUrl.Replace("github.com", "raw.githubusercontent.com").Replace("/blob/", "/");
        }

        private string GetCodeSample(string code)
        {
            var lines = code.Split('\n', '\r')
                .Where(x => 
                { 
                    var trimmedString = x.Trim(); 
                    return !(string.IsNullOrEmpty(trimmedString) || trimmedString.StartsWith("//", "/*", "*", "#", "--")); 
                })
                .ToList();

            if (lines.Count <= LinesCount)
            {
                return string.Join("\n", lines);
            }

            var startIndex = _random.Next(0, lines.Count - LinesCount);
            var codeLines = lines.Skip(startIndex).Take(LinesCount);

            return string.Join("\n", codeLines);
        }
    }
}
