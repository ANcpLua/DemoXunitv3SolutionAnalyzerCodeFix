using System;
using ANcpLua.Analyzers.Analyzers;
using ANcpLua.Analyzers.CodeFixes.CodeFixes;
using ANcpLua.Roslyn.Utilities.Testing;
using Xunit;

namespace DemoXunitv3SolutionAnalyzerCodeFix;

public sealed class CancellationAnalyzerSmokeTests
    : CodeFixTest<Al0126CancellationTokenPropagationAnalyzer, Al0126CancellationTokenPropagationCodeFixProvider>
{
    [Fact]
    public async Task FixProvider_replaces_default_token_with_xunit_test_context_token()
    {
        const string source = """
            using System;
            using System.Threading;
            using System.Threading.Tasks;
            using Xunit;

            namespace Xunit
            {
                public sealed class FactAttribute : Attribute { }

                public sealed class TestContext
                {
                    public static TestContext Current { get; } = new TestContext();
                    public CancellationToken CancellationToken { get; } = new CancellationToken();
                }
            }

            public sealed class UnitTests
            {
                [Fact]
                public async Task Executes() =>
                    await {|AL0126:DoThingAsync(default)|};
            
                private static Task DoThingAsync(CancellationToken cancellationToken) =>
                    Task.CompletedTask;
            }
            """;

        const string fixedSource = """
            using System;
            using System.Threading;
            using System.Threading.Tasks;
            using Xunit;
            
            namespace Xunit
            {
                public sealed class FactAttribute : Attribute { }
            
                public sealed class TestContext
                {
                    public static TestContext Current { get; } = new TestContext();
                    public CancellationToken CancellationToken { get; } = new CancellationToken();
                }
            }

            public sealed class UnitTests
            {
                [Fact]
                public async Task Executes() =>
                    await DoThingAsync(global::Xunit.TestContext.Current.CancellationToken);
            
                private static Task DoThingAsync(CancellationToken cancellationToken) =>
                    Task.CompletedTask;
            }
            """;

        await VerifyAsync(source, fixedSource);
    }

    [Fact]
    public void Demo_uses_real_analyzer_rule()
    {
        Assert.Equal("AL0126", Al0126CancellationTokenPropagationAnalyzer.DiagnosticId);
    }
}
