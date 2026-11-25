
% i am a comment
% me too

function [A] = F1 (b, c)

if b > 1,
	if c > 3,
		A = f3 (1);
		disp ('1111')
	else
		A = f3 (10);
		disp ('2222')
	end
	
else
	if c > 5,
		A = f3 (6);
		disp ('3333')
	else
		A = f3 (60);
		disp ('4444')
	end
end




% [pp, qq] = f2 (5, 6, 7);

% disp (sprintf ('print inside F1, A = %d', A));
