namespace ReleaseNoteBuilder.Services;
public class ReleaseService{
public string Generate(System.Collections.Generic.List<object> items,string env,int buildId){
return $"Release {env} Build {buildId}";}}
