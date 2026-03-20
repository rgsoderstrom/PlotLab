
namespace PLCommon
{
    public delegate PLVariable PLFunction    (PLVariable var);
    public delegate void       PrintFunction (string str);

    public enum SymbolicNameTypes
    {
        Unknown, 
        Variable, 
        Constant,
        
        WorkspaceCommand, 
        Function, 

        //  WorkspaceFunction,
        //ScriptFile, 
        FunctionFile,       // single output function
        //FunctionFile_Multi, // multiple output function file
        PlotCommand, 
        //SystemCommand, 
        //FlowCtrl,
    };

    //public enum Results  // DO I NEED THIS???
    //{
    //    NotFound, Handled, Failed
    //}
}
