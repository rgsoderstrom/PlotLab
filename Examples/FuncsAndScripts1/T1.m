
% T1.m

theta = linspace (0, 2*pi, 64);
r1 = 0.75;

x1 = r1 * cos (theta);
y1 = r1 * sin (theta);

clf ; hold on ; plot (x1, y1);

r2 = 0.25;
x2 = r2 * cos (theta);
y2 = r2 * sin (theta);

r3 = 0.938;
theta2 = (0 : 22.5 : 359) * pi / 180;

for t = theta2,
	xc = r3 * cos (t);
	yc = r3 * sin (t);
	
	plot (xc + x2, yc + y2, 'r');
end

axis equal

% disp ('running t1');

% A = [1, 2, 3 ; ...
     % 4, 5, 6 ; ...
	 % 7, 8, 9];
      
% disp ('go to T2')	  
% t2     
% disp ('back from T2')	  
   
% B = 2
% C = 3;
% disp ('t1 complete')


