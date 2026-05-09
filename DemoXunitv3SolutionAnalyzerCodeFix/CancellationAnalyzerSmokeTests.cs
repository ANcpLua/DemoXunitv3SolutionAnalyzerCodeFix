using System;
using ANcpLua.Roslyn.Utilities.Examples.XunitCancellationAnalyzer;
using ANcpLua.Roslyn.Utilities.Examples.XunitCancellationCodeFixes;
using ANcpLua.Roslyn.Utilities.Examples.XunitCancellationShared;
using ANcpLua.Roslyn.Utilities.Testing;
using Xunit;

namespace DemoXunitv3SolutionAnalyzerCodeFix;

public sealed class CancellationAnalyzerSmokeTests : CodeFixTest<MissingCancellationTokenAnalyzer, MissingCancellationTokenFixer>
{
    [Fact]
    public async Task FixProvider_adds_test_context_cancellation_token_on_omitted_argument()
    {
        const string source = """
            using System;
            using System.Threading;
            using System.Threading.Tasks;
            using Xunit;

            namespace Xunit
            {
                public sealed class FactAttribute : Attribute { }
            }

            public sealed class UnitTests
            {
                [Fact]
                public async Task Executes()
                {
                    await {|ANCP0001:DoThingAsync(default)|};
                }

                private static Task DoThingAsync(CancellationToken cancellationToken) =>
                    Task.CompletedTask;
            }
            public static class TestContext
            {
                public static ContextState Current { get; } = new ContextState();
            }

            public sealed class ContextState
            {
                public CancellationToken CancellationToken { get; } = new CancellationToken();
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
            }

            public sealed class UnitTests
            {
                [Fact]
                public async Task Executes()
                {
                    await DoThingAsync(TestContext.Current.CancellationToken);
                }

                private static Task DoThingAsync(CancellationToken cancellationToken) =>
                    Task.CompletedTask;
            }
            public static class TestContext
            {
                public static ContextState Current { get; } = new ContextState();
            }

            public sealed class ContextState
            {
                public CancellationToken CancellationToken { get; } = new CancellationToken();
            }
            """;

        await VerifyAsync(source, fixedSource);
    }

    [Fact]
    public void Shared_contract_uses_expected_replacement_expression()
    {
        Assert.Equal("TestContext.Current.CancellationToken", MissingCancellationTokenContract.ReplacementExpression);
        Assert.NotNull(MissingCancellationTokenContract.CreateReplacementTokenExpression());
    }
}
