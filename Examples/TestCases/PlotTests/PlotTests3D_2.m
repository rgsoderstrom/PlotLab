
% PlotTests2D_1.m - Point cloud in 3D space

N = 20; % number of points

pts = 15 * rand (3, N);

hold off
plot (pts, '*', 'Size', 0.8);
hold on
plot (pts);

title ('Random Points')

%PlotCenter (1, 2, 3);
PlotCenter (pts (:, 2));
%PlotCenter (pts (1, 1), pts (2, 1), pts (3, 1));

CameraPos (8, 4, -2);

