
% PlotTests2D_1.m


% plot (2, 3, 'r', 'Size', 0.3, 'Thickness', 5);
% plot ([3;1], 'Size', 0.15, 'Thickness', 6);
% plot (1, 2, 'g*', 'Size', 0.2, 'Thickness', 7);
% axis ([0 4 0 5]);
% title ('Individual points');

% x = [-3 : 0.2 : 3];
% y = x .^ 3;

% figure ; plot (y); title ('x axis equals index, 1..N');
% figure ; plot (x, y); title ('x axis values passed to plot');
% figure ; plot (x, y, 'g*', 'MarkerSize', 0.1); title ('discrete points');

xx = [2 : 0.5 : 8];
yy = log (xx);
pts = [xx ; yy];
figure ; hold on
plot (pts) ; plot (pts, 'ro', 'MarkerSize', 0.2);
title ('Points on top of line');



