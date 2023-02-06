
% Contour1 

% contour (xValues, yValues, zValues, levels)
%      - args 1 & 2 are lists
%      - arg 3 is a matrix
%      - arg 4 is a list

NR = 20; % number rows. also number of Y values
NC = 25; % number cols. also number of X values

xValues = linspace (-2, 2, NC);
yValues = linspace (0, 4, NR);
zValues = yValues' * xValues; % size NR rows by NC cols

if (size (zValues, 1) ~= NR) || (size (zValues, 2) ~= NC)
	disp ('Size error');
end

%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

% default 5 equally spaced levels

%contour (xValues, yValues, zValues);


%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

% specify levels or the number of levels

levels = -4 : 4;
%levels = linspace (-4, 4, 8);
%levels = 9;

%contour (xValues, yValues, zValues, levels);

%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

% Background colors

% levels = 9;
% contour (xValues, yValues, zValues, levels, 'ShowBackground', 1);

%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

% Don't show contour lines

%contour (xValues, yValues, zValues, 'ShowBackground', 1, 'DrawContourLines', 'false');

%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

% Colored contour lines with gradient arrows

%contour (xValues, yValues, zValues, 1:5, 'DrawLinesInColors', 'true', 'DrawArrows', 1);

%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

% Contour lines labelled

contour (xValues, yValues, zValues, 1:5, 'LabelLines', 'true', 'LabelFontSize', 0.1);
















