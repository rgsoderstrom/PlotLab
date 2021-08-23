

% p1.m

x = [-5 : 0.5 : 5]; 
y = x .^ 2;

% plot (x, y, '--');
% hold on
% pause 'long dash'

h = plot (x, y + 1, 'g+');

set (h, 'Size', 1);
set (h, 'Thickness', 4);

% pause 'green plus'

% plot (x, y + 2, 'r*');
% pause ('red star')

[11:21]
