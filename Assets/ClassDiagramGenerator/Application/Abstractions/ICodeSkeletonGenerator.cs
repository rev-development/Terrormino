namespace ClassDiagramGenerator.Application.Abstractions
{
    public interface ICodeSkeletonGenerator
    {
        GenerateSkeletonResult Generate(GenerateSkeletonRequest request);
    }
}