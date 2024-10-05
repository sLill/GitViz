namespace GitViz.Core;
public static class GitUtils
{
    #region Methods..
    public static int GetCommitCount(string repositoryPath, string? branch = null)
    {
        int totalCommits = 0;

        using (var repository = new Repository(repositoryPath))
        {
            if (branch != null)
                totalCommits = repository.Branches[branch].Commits.Count();
            else 
                totalCommits = repository.Commits.Count();
        }

        return totalCommits;
    }

    public static IEnumerator GetAllCommits(string repositoryPath, string? branch = null)
    {
        using (var repository = new Repository(repositoryPath))
        {
            if (branch != null)
                yield return repository.Branches[branch].Commits;
            else
                yield return repository.Commits;
        }
    }

    public static (int LinesAdded, int LinesDeleted) GetCommitChanges(string repositoryPath, string commitSha)
    {
        using (var repository = new Repository(repositoryPath))
        {
            var commit = repository.Commits.FirstOrDefault(c => c.Sha.StartsWith(commitSha));
            if (commit == null)
                throw new ArgumentException($"Commit with SHA {commitSha} not found.");

            var parent = commit.Parents.FirstOrDefault();
            if (parent == null)
                return (commit.Tree.Count, 0);

            var compareOptions = new CompareOptions { Similarity = new SimilarityOptions() { WhitespaceMode = WhitespaceMode.IgnoreAllWhitespace } };
            var patch = repository.Diff.Compare<Patch>(parent.Tree, commit.Tree, compareOptions);
            int linesAdded = 0;
            int linesDeleted = 0;

            foreach (var change in patch)
            {
                linesAdded += change.LinesAdded;
                linesDeleted += change.LinesDeleted;
            }

            return (linesAdded, linesDeleted);
        }
    }

    public static Dictionary<string, (int LinesAdded, int LinesDeleted)> GetCommitFileChanges(string repositoryPath, string commitSha)
    {
        using (var repo = new Repository(repositoryPath))
        {
            var commit = repo.Commits.FirstOrDefault(c => c.Sha.StartsWith(commitSha));
            if (commit == null)
                throw new ArgumentException($"Commit with SHA {commitSha} not found.");

            var parent = commit.Parents.FirstOrDefault();
            if (parent == null)
            {
                // Initial commit
                return commit.Tree.Select(entry => new
                {
                    Path = entry.Path,
                    Changes = (LinesAdded: GetFileLineCount(entry.Target as Blob), LinesDeleted: 0)
                }).ToDictionary(x => x.Path, x => x.Changes);
            }

            var compareOptions = new CompareOptions { Similarity = new SimilarityOptions() { WhitespaceMode = WhitespaceMode.IgnoreAllWhitespace } };
            var patch = repo.Diff.Compare<Patch>(parent.Tree, commit.Tree, compareOptions);
            var fileChanges = new Dictionary<string, (int LinesAdded, int LinesDeleted)>();

            foreach (var change in patch)
                fileChanges[change.Path] = (change.LinesAdded, change.LinesDeleted);

            return fileChanges;
        }
    }

    public static Dictionary<DateTime, (int LinesAdded, int LinesDeleted)> GetMonthlyChanges(string repositoryPath, bool excludeWhitespace = true)
    {
        //var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        using (var repository = new Repository(repositoryPath))
        {
            var monthlyChanges = new ConcurrentDictionary<DateTime, (int LinesAdded, int LinesDeleted)>();
            var compareOptions = new CompareOptions { Similarity = new SimilarityOptions() { WhitespaceMode = excludeWhitespace ? WhitespaceMode.IgnoreAllWhitespace : WhitespaceMode.DontIgnoreWhitespace } };

            Parallel.ForEach(repository.Commits, commit =>
            {
                var commitMonth = new DateTime(commit.Author.When.Year, commit.Author.When.Month, 1);

                var parent = commit.Parents.FirstOrDefault();
                if (parent == null)
                {
                    // Initial commit
                    var initialLines = CountLinesInCommit(repository, commit);
                    monthlyChanges.AddOrUpdate(commitMonth,
                        (key) => (initialLines, 0),
                        (key, old) => (old.LinesAdded + initialLines, old.LinesDeleted));
                }
                else
                {
                    var patch = repository.Diff.Compare<Patch>(parent.Tree, commit.Tree, compareOptions);
                    var linesAdded = patch.Sum(change => change.LinesAdded);
                    var linesDeleted = patch.Sum(change => change.LinesDeleted);

                    monthlyChanges.AddOrUpdate(commitMonth,
                        (key) => (linesAdded, linesDeleted),
                        (key, old) => (old.LinesAdded + linesAdded, old.LinesDeleted + linesDeleted));
                }
            });

            //stopwatch.Stop();

            return new Dictionary<DateTime, (int LinesAdded, int LinesDeleted)>(monthlyChanges.OrderBy(x => x.Key));
        }
    }

    private static int CountLinesInCommit(Repository repo, Commit commit)
    {
        int totalLines = 0;
        foreach (var entry in commit.Tree)
        {
            if (entry.TargetType == TreeEntryTargetType.Blob)
            {
                var blob = (Blob)entry.Target;
                totalLines += GetFileLineCount(blob);
            }
        }
        return totalLines;
    }

    private static int GetFileLineCount(Blob blob)
    {
        if (blob == null) 
            return 0;

        using (var contentStream = blob.GetContentStream())
        using (var streamReader = new StreamReader(contentStream))
        {
            int lineCount = 0;
            
            while (streamReader.ReadLine() != null) 
                lineCount++;
            
            return lineCount;
        }
    }
    #endregion Methods..
}
