namespace DevOpsAppService.EggApi;

public sealed class EggApiOptions
{
    public const string SectionName = "EggApi";

    public string AuxbrainBaseUrl { get; init; } = "https://ctx-dot-auxbrainhome.appspot.com";
    public uint? ClientVersion { get; init; }
}
