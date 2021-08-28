
% PlotTests2D_1.m

% plot (2, 3);
% plot ([3;2]);
% plot (1, 2, '*');

x = [-3 : 0.2 : 3];
y = x .^ 3;

% figure ; plot (y);
% figure ; plot (x, y);
% figure ; plot (y, 'r');
% figure ; plot (x, y, 'g*');

% xx = [2 : 0.5 : 8];
% yy = log (xx);
% pts = [xx ; yy];
% figure ; plot (pts);
% figure ; plot (pts, 'r*');


plot (x, y, 'LineWidth', 3, 'Color', 'r');
