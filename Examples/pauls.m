X = [-5:0.5:5];
Y = [-5:0.5:5];

z = zeros (length (Y), length (X));

for c = 1: length (X),
	for r = 1 : length (Y),
		z (r, c) = 1/4 * X (c) - 1/2 * Y (r) * Y (r);
	end
end
	
%z
	
contour (X, Y, z, [-13:1:2])
