namespace DevOpsAppApi;

public class FeatureFlagsOptions
{
    public const string SectionName = "FeatureFlags";

    public bool Leaderboards { get; set; } = true;
    public bool Admin { get; set; } = true;
}
