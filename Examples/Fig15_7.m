
% H&K Figure 15-7

w = 0 : 0.2 : 10;

z1 = j * w;         % zero
p1 = j * w + 0.826; % pole 1
p2 = j * w + 25.5;  % pole 2

den = p1 .* p2;

gain = 189.5 * z1 ./ den;
