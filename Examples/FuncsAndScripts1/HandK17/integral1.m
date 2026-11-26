
% test driver for integrate function

t0 = 0;
T  = 2*pi;
N  = 31; % number of points, odd for best results.
		 % number of intervals is one less

figure (1) ; clf ;

x = linspace (t0, T, N);
dx = (T - t0) / (N - 1); % interval width

% y = x .* x;

n = 1;
k = 1;

y1 = sin (n * x);
y2 = sin (k * x);
y  = y1 .* y2;

clear res stat

clf 
plot (x, y) ; hold on ; plot (x, y, '*', 'r', 'Size', dx / 6);
[res, stat] = integrate (y, dx);
disp (sprintf ('Results = %3.3f', res));

if stat ~= 0,
	disp (sprintf ('Status  = %f', stat));
end
