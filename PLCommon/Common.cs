
namespace PLCommon
{
    public delegate PLVariable PLFunction (PLVariable var);
    public delegate bool       BSFunction (string str);
    public delegate PLVariable PZFunction ();  // return PLVariable, no input args

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

        ZeroArgFunction,
        Function,
        BlockStart, // for, while, if
        BlockEnd,
        ScriptFile,
        FunctionFile, 
    };

    public enum InputLineType
    {
        Unknown,
        ExpressionTree, 
        VariableName,
        ZeroArgFunction,
        SystemCommand,
        PlotCommand,
        WorkspaceCommand,
        ScriptFile,
        BlockStart,
        BlockEnd,
        BlockName,
    }
}
