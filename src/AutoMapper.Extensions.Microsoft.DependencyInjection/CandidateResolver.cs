#if DEPENDENCY_MODEL

namespace AutoMapper
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Collections.Generic;
    using Microsoft.Extensions.DependencyModel;

    internal class CandidateResolver
    {
        private static HashSet<string> ReferenceAssemblies { get; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "AutoMapper"
        };

        private readonly DependencyContext _dependencyContext;
        private readonly IDictionary<string, Dependency> _dependencies;

        public CandidateResolver(DependencyContext dependencyContext)
        {
            var dependenciesWithNoDuplicates = new Dictionary<string, Dependency>(StringComparer.OrdinalIgnoreCase);
            foreach (var dependency in dependencyContext.RuntimeLibraries)
            {
                if (dependenciesWithNoDuplicates.ContainsKey(dependency.Name))
                {
                    throw new InvalidOperationException(
                        $"A duplicate entry for library reference {dependency.Name} was found. Please check that all package references in all projects use the same casing for the same package references.");
                }
                dependenciesWithNoDuplicates.Add(dependency.Name, CreateDependency(dependency));
            }

            _dependencyContext = dependencyContext;
            _dependencies = dependenciesWithNoDuplicates;
        }

        private Dependency CreateDependency(RuntimeLibrary library)
        {
            var classification = DependencyClassification.Unknown;
            if (ReferenceAssemblies.Contains(library.Name))
            {
                classification = DependencyClassification.AutoMapperReference;
            }

            return new Dependency(library, classification);
        }

        private DependencyClassification ComputeClassification(string dependency)
        {
            var candidateEntry = _dependencies[dependency];
            if (candidateEntry.Classification != DependencyClassification.Unknown)
            {
                return candidateEntry.Classification;
            }
            else
            {
                var classification = DependencyClassification.NotCandidate;
                foreach (var candidateDependency in candidateEntry.Library.Dependencies)
                {
                    var dependencyClassification = ComputeClassification(candidateDependency.Name);
                    if (dependencyClassification == DependencyClassification.Candidate ||
                        dependencyClassification == DependencyClassification.AutoMapperReference)
                    {
                        classification = DependencyClassification.Candidate;
                        break;
                    }
                }

                candidateEntry.Classification = classification;

                return classification;
            }
        }

        public IEnumerable<RuntimeLibrary> GetCandidateLibraries()
        {
            foreach (var dependency in _dependencies)
            {
                if (ComputeClassification(dependency.Key) == DependencyClassification.Candidate)
                {
                    yield return dependency.Value.Library;
                }
            }
        }

        public IEnumerable<Assembly> GetCandidateAssemblies()
        {
            return GetCandidateLibraries()
                .SelectMany(library => library.GetDefaultAssemblyNames(_dependencyContext))
                .Select(Assembly.Load);
        }

        private class Dependency
        {
            public Dependency(RuntimeLibrary library, DependencyClassification classification)
            {
                Library = library;
                Classification = classification;
            }

            public RuntimeLibrary Library { get; }

            public DependencyClassification Classification { get; set; }

            public override string ToString()
            {
                return $"Library: {Library.Name}, Classification: {Classification}";
            }
        }

        private enum DependencyClassification
        {
            Unknown = 0,
            Candidate = 1,
            NotCandidate = 2,
            AutoMapperReference = 3
        }
    }
}

#endif