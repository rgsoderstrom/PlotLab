
namespace PLCommon
{
    public delegate PLVariable PLFunction    (PLVariable var);
    public delegate void       PrintFunction (string str);
    public delegate bool       PLRequest     (string str);

    public enum SymbolicNameTypes
    {
        Unknown,
        Variable,
        Constant,

        WorkspaceCommand,
        PlotCommand,
        SystemCommand,

        Function,

        ScriptFile,
        FunctionFile, 

    };

    public enum InputLineType
    {
        Unknown,
        ExpressionTree, 
        VariableName,
        SystemCommand,
        PlotCommand,
        ScriptFile,
        FunctionFile,
        BlockStart,
        BlockEnd,
    }
}
