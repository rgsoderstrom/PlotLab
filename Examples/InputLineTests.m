

% InputLine tests	
	% b = 7 + ...  % zzzz
	    % 8 + ...
		% 9;	

	% c = [1 ; ...
	     % 2 ; ...
		 % 3];
		 
	% d = [10, ...
	     % 20, ...
		 % 30];
		 
	% a	
	% startup	
	% path	
	% figure
	
	% j=3; k = 7; l=  9;
		
	%
	% Blocks
	%

	% if A > B,       % BLOCK_1, test [0]
		% c = A * B;
		
		% if A > 19,  % BLOCK_2, test [0]
			% c = 3;
		% end

		% c = -4;

		% if Z > 33,  % BLOCK_3, test [0]
			% c = 3;
			
		% else        % BLOCK_3, test [1]
			% c = 99;
		% end

		% c = -1;
	% end


	% while A > B,
		% if C > D,
			% e = 3;
		% elseif E > F
			% f = 2;
		% else
			% g = 1;
			% break;
		% end
		
		% h = 0;
		% %break;	
	% end

	% while A > B,
		% while C > D,
			% f = 2;
		% %	break;
		% end
		
		% h = 0;
	% end


% % INFINITE LOOP
	% while a < 2,
		% while b < 3,
			% if c < 2,
				% a = a + 1;
			% else
				% b = b + 1;
				% %break;
			% end
		% end
		
		% c = c + 1;
	% end


	while a < 8,
		a = a + 1;
		b = b + 1;
		
		if b > 12,
			continue;
		end
		
		c = c + 1;
	end


	% for a = 1:9
		% b = a ^ 2; 
		
		% for z = 11 : 18,
			% x = z ^ 3;
		% end
		
		% disp (b);
	% end
