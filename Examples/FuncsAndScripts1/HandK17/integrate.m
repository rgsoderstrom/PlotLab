
% integrate.m

% Simpson's 1/3 rule, from Google's AI Summary
% 	web article uses 0-based indexing

% Simpson's requires an even number of intervals 
%	i.e. length (y) is odd

function [results] = integrate (y, dx)

N = length (y);

if N < 5,
	disp ('integrate.m - too few points, requires at least 5');
	results = 0;
	return;
end

evenNumberOfIntervals = (floor (N / 2) * 2 ~= N);

if evenNumberOfIntervals == false,
	disp ('integrate.m - requires an odd number of input points');
	results = 0;
	return;
end

first = 1;
last  = N;

weightByOne  = [first, last];
weightByFour = (first + 1) : 2 : (last - 1);
weightByTwo  = (first + 2) : 2 : (last - 2);

% disp (sprintf ('weight 1: %d', weightByOne));
% disp (sprintf ('weight 4: %d', weightByFour));
% disp (sprintf ('weight 2: %d', weightByTwo));

summation = 0;
summation = summation + sum (y (weightByOne));
summation = summation + sum (y (weightByTwo))  * 2;
summation = summation + sum (y (weightByFour)) * 4;

results = summation * dx / 3;
