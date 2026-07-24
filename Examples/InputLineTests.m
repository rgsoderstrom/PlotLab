

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

	if A > B,       % BLOCK_1, test [0]
		c = A * B;
		
		if A > 19,  % BLOCK_2, test [0]
			c = 3;
		end

		c = -4;

		if A > 33,  % BLOCK_3, test [0]
			c = 3;
			
		else        % BLOCK_3, test [1]
			c = 99;
		end

		c = -1;
	end


	% if A > B,      % BLOCK_1, test [0]
		% c = A * B;
		
	% elseif A > 19, % BLOCK_1, test [1]
		% c = 3;
		
	% else           % BLOCK_1, test [2]
		% c = -1;
	% end

	% if A > B, c = A * B; disp (c); end
	

	% for a = 1:9
		% b = a ^ 2; 
		% disp (b);
	% end



	% for a = 1:9
		% b = a ^ 2; 
		
		% for z = 11 : 18,
			% x = z ^ 3;
		% end
		
		% disp (b);
	% end
