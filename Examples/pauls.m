X = [-5 : 0.25 : 5];
Y = [-4 : 0.25 : 4];

z = zeros (length (Y), length (X));

for c = 1: length (X),
	for r = 1 : length (Y),
		z (r, c) = 1/4 * X (c) - 1/2 * Y (r) * Y (r);
	end
end
	
%contour (z, 'Levels', [-8 : 1 : 1]);
%contour (z, [-8 : 1 : 1], 'ShowBackground', 'tru0e');

contour (X, Y, z, 6, ... % linspace (-12, 2, 6), ...
				  'ShowBackground', 1, ...
                  'DrawContourLines', 1, ...
				  'DrawLinesInColors', 0, ...
				  'DrawArrows', 1, ...
				  'LabelLines', 1);

