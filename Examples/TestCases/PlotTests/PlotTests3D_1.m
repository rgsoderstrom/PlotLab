
% PlotTests3D_1.m - Points in 3D space

% figure (1) ; hold on
% plot (rand (3, 1), 'r', 'Size', 0.1);
% plot (rand (3, 1), 'g', 'Size', 0.15);
% plot (rand (3, 1), 'b', 'Size', 0.2);
% plot (rand (3, 1), '',  'Size', 0.25);

% title ('Random points, different sizes')

clf

t = 1 : 0.1 : 8 * pi;
x = (6 - t/8) .* cos (t);
y = (5 - t/8) .* sin (t);
z = 3 * log (t);

pts = [x ; y ; z];
figure (2) % ; plot (pts);

hold on

select = 1 : 8 : size (pts, 2);

for c = select,
	v = [pts(1, c) ; pts(2, c) ; pts(3, c)];
	plot (v, 'g', 'Size', 0.25);
end

