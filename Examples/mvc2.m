
% mvc2.m - scratch file for MVC problems

% D* space
%figure (1); hold on ; set (1, 'Position', [100 10 750 550]);

% D space
figure (2); clf ; hold on ; set (2, 'Position', [1000 10 750 550]);

rs = linspace (0.25, 2, 4);
ts = linspace (0, 2*pi, 20);

for ri = 1 : length (rs),
	r = rs (ri);

	for ti = 1 : length (ts),
		t = ts (ti);
		
		x = r * cos (t) + r * sin (t);
		y = (1/2) * r * sin (t);
		
		plot (x, y, '+', 'Size', 0.05);
	end
end

axis equal

clear r t x y

%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

r1 = 1;
t1 = pi / 7;

x1 = r1 * cos (t1) + r1 * sin (t1);
y1 = (1/2) * r1 * sin (t1);

plot (x1, y1, 'r*', 'Size', 0.05);


dr = -0.05;
dt = 0.09;

r2 = r1 + dr;
t2 = t1 + dt;

x2 = r2 * cos (t2) + r2 * sin (t2);
y2 = (1/2) * r2 * sin (t2);

plot (x2, y2, 'g*', 'Size', 0.05);

%%%%%%%%%%%%%%%%%%%%%%%%%

DT = [(cos (t1) + sin (t1)) (-r1 * sin (t1) + r1 * cos (t1)) ; ((1/2) * sin (t1)) ((1/2) * r1 * cos (t1))];

v = DT * [dr ; dt];

disp (v);
disp ('');

disp ([(x2 - x1) ; (y2 - y1)]);

plot (x1 + v (1), y1 + v (2), 'b*', 'Size', 0.05);






