
% PlotTests3D_3.m - surface in 3D space

x = [-4 : 1 : 4]; % row vectors
y = [-3 : 1 : 3];

xm = ones (length (y), 1) * x;
ym = y' * ones (1, length (x));

xm2 = xm .* xm;
ym2 = ym .^ 2;

z = 0.25 * (xm2 + ym2 - 6) - 1;

plot (x, y, z);
title ('Paraboloid');

