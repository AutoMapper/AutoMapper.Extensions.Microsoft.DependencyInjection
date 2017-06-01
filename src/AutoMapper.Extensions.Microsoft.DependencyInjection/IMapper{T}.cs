namespace AutoMapper
{
    /// <summary>
    /// <see cref="IMapper"/> that allows the specification of a <see cref="IConfigurationProvider"/> implementation
    /// </summary>
    /// <typeparam name="T">The <see cref="IConfigurationProvider"/> implementation used to configure the mapper</typeparam>
    /// <remarks>
    /// This interface enables scenarios where multiple configurations for the same src/dst type pairs are required.
    /// </remarks>
    // ReSharper disable once UnusedTypeParameter
    public interface IMapper<T> : IMapper
        where T : IConfigurationProvider
    {
    }
}