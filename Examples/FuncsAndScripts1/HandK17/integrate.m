
% integrate.m

% Simpson's 1/3 rule, from Google's AI Summary
% 	web article uses 0-based indexing

% Simpson's requires an even number of intervals 
%	i.e. length (y) is odd

% for an odd number of intervals the last one
% is excluded from Simpson's and evaluated by
% a rectangular approximation

% returns:
%	results - intergration estimate
%	status  - 0 no error or warning
%			  1 low accuracy due to odd number of intervals
%			  2 error due to not enough input y

function [results, status] = integrate (y, dx)

N = length (y);

if N < 5,
	status = 2; % error
	results = 0;
	return;
end

evenNumberOfIntervals = (floor (N / 2) * 2 ~= N);

first = 1; % always start with first y

if evenNumberOfIntervals,
	last = N;
	status = 0;
else
	last = N - 1;
	status = 1;
end

weightByOne  = [first, last];
weightByFour = (first + 1) : 2 : (last - 1);
weightByTwo  = (first + 2) : 2 : (last - 2);

% disp (sprintf ('weight 1: %d', weightByOne));
% disp (sprintf ('weight 4: %d', weightByFour));
% disp (sprintf ('weight 2: %d', weightByTwo));
% disp (sprintf ('Is even:  %d', evenNumberOfIntervals));

summation = 0;
summation = summation + sum (y (weightByOne));
summation = summation + sum (y (weightByTwo))  * 2;
summation = summation + sum (y (weightByFour)) * 4;

results = summation * dx / 3;

% if evenNumberOfIntervals == false,
	% results = results + mean (y (N-1:N)) * dx;
% end





