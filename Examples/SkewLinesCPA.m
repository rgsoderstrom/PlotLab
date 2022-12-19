
% SkewLinesCPA - find closest point of two parameterized 3D lines

% line 1 start point and velocity vector
p1 = [-1 ;  0 ; 1];
v1 = [ 1 ; -3 ; 2];

% line 2 start point and velocity vector
p2 = [ 1 ; 5 ; 4];
v2 = [-1 ; 1 ; 6];


% p1 = [2 ; 1 ; 0];
% v1 = [0 ; 0 ; 1];

% p2 = [0 ; 1 ; 3];
% v2 = [1 ; 0 ; 0];

%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

% solve p1 + t1 * v1 = p2 + t2 * v2;
% for t1 and t2

% p1 - p2 = t2 * v2 - t1 * v1;

% in matrix form: At = b;

b = p1 - p2;
A = [-v1 v2];

t = inv (A' * A) * A' * b;

disp ('Line 1 point:');
disp (p1 + t (1) * v1);
disp ('');
disp ('Line 1 time parameter');
disp (t (1));
disp ('');

disp ('Line 2 point:');
disp (p2 + t (2) * v2);
disp ('');
disp ('Line 2 time parameter');
disp (t (2));


N = 40;
tt = linspace (-2, 2, N);

r1 = p1 * ones (1, N) + v1 * tt;
r2 = p2 * ones (1, N) + v2 * tt;

plot (r1);
plot (r2);


plot (p1 + t (1) * v1, 'Size', 1);
plot (p2 + t (2) * v2, 'Size', 1);



