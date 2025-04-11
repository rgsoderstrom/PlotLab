
% curves.m

ti = 0;
tf = 10;
t = linspace (ti, tf, 101);

x = t .^ 2;
y = t .^ 2;

plot (x, y);

select = 1 : 20 : length (t);

for s = select,
	plot (x (s), y (s), 'Size', 1);
end


