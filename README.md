
PlotLab Repository
==================

PlotLab is a scripting environment that uses the plotting tools in the PlotxD_Embedded and PlottingLib. Syntax is similar to Matlab.

Main - user interface and script processing. Expression evaluator and functions to support it.

PLCommon - defines the "PL" variables used in most of the other code. Top level
		   abstract class is PLVariable and all other derived from it.

PLLibrary - implementations of most functions invoked by user's script (e.g. sin, plot, etc)

PLWorkspace - manages the user's workspace. Also predefined constants (e.g. PI)

Examples - script files used to test PlotLab functionality

ut directories - unit tests of various modules

zDocumentation - start of documentation. NOT MAINTAINED, LIKELY NO LONGER ACCURATE.

PetzoldMedia3D - available at http://www.charlespetzold.com/3D/


Combined into "Main" and individual projects removed:
FrontEnd - user interface and script processing
PLKernel - expression evaluator and functions to support it.


