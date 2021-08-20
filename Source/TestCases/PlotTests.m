
% PlotTests.m

x = [-3 : 0.2 : 3];
y = x .^ 2;
z = x .^ 3;

xy  = [x ; y];
xyz = [x ; y ; z];

plot (x, y);
pause

clf ; plot (x, y, 'r*');


