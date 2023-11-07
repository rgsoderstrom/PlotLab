
% Rotate2D

% columns of E1 are initial basis vectors. Need not be orthogonal
%E1 = [1 0 ; 0 1]; 
E1 = [0.9 0.1 ; -0.2 0.8]; 
V1 = [4;3];
PlotVector (V1, E1, 'rc');

t  = 30 * pi / 180;
R  = [cos(t) -sin(t) ; sin(t) cos(t)];
Ri = inv (R);

E2 = E1 * R;
V2 = Ri * V1;
PlotVector (V2, E2, '.c');

title ('Vector in 2 bases');
