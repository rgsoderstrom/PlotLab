
% PlotTests3D_4.m - surface in 3D space

% sin (r) / r

halfLength = 5;
halfWidth = halfLength * 0.75;
step = halfWidth / 16;

x = [-halfLength : step : halfLength]; % row vectors
y = [-halfWidth  : step : halfWidth];

% make them into matrices that vary along one dimension only
xm = ones (length (y), 1) * x;
ym = y' * ones (1, length (x));

xm2 = xm .* xm;
ym2 = ym .^ 2;
d   = sqrt (xm2 + ym2);
z   = 0.25 * sin (4 * d) ./ d;

plot (x, y, z);
title ('0.25 * sin (4r) / r');
axis frozen
grid on
CameraRelPos (2, 3, 4);




