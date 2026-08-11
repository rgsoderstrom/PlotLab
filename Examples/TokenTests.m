

% TokenTests.m, for utTokens

	
	% AnnotatedString - alphanumeric only
	% BLOCK_2
	% BLOCK_T
	[3 : 12]
	3 : 2 : 12
	
	% clear a b c
	% for a = 1:10,
	% while (b > 2)
	% startup
	% figure
	
	% AnnotatedString - 
	% if A > B,
	% if A > B, c = A * B; disp (c); end
	% if z + x, a = [1,2,3]
	% p = [1 ; (sqrt (5 * 3)) ; 7]
	% p=[1 ; 2 ; 3 ; 4 ; (sqrt (5 * 3)) ; 6 ; 7]
	% d = [1 ; (2 * 7) ; 3]'
	% x >= 3;	
	% y = z12' .* xyzx';
	% y = z12' .* xyzx'
	% z = A' + [1:2:20]'
	
	% a = b123 + 1;
	% d = sin ((10 - 7) / 7);
	% e = 22 / 7
	% e = 22 / 7;
 	% z = 3 + 4 * sin (P') * 5;
	% % a = b' + c' + d' + e';
	
	% d22 = 'zx+3v';
	% e = 'aaa' + 'bbb' + 'ccc' + 8;
	% z = 'a.a\'b';
	% a = b4';
	
	% ab12 = -123.456e-7;
	% p = 1:3:30;
	% q = [1 ; a ; 3];
	% b6 = (1, 2, 3, 4)';
	% abc123 = sprintf ('%d', [1 ; 2 ; 3]);
	% -12
	% ab = sprintf ('%f', sin (c));
	% c = sprintf ('aa \'bbb\' cc');
	
	% AnnotatedStringSet - 
	% a = 4; b = a ^ 2; 
	% a = 4; b = a ^ 2; c = b ^ 2; d = c / 3; clear a b c
	
	
	% TokenParsing
	% a = sprintf ('%3d', 123);
	% disp (a);
	% a = '%3d'
	% '%3d'
	% p = [1 ; 2 ; 3 ; 4 ; 5 ; 6 ; 7];
	% z = 987 * 23;
	% z = -987;
	% c = ~c123 & 1;
	% d = -3 * -sin (4);
	% t1 = [1 2 3]' + (4 : 6)' + [7:9]' + dd' + c123;
	% b6 = (1, 2, 3, 4)';
	% a4 = [1, sqrt (2), -3]'
	% zz = 'asd' + 'fg';

	% TokenUtils
	% [2;sin (z) ; 6^2]
	% [[1:3]; [2:4] ; [3:5] ]
	% [(1 ; 3) ; [2 ; 4] ;[3 ;5] ]
	
	% [Abc, 456, 789]
 	% [1,2,3]
	% [11, 12 , 13]
	% ['a,aa', 'sss']
	% ['a:aa', 'sss']
	
	% [4 sqrt(5) 6]
	

	% [1 : 3 : 20]
	% [1:ZZ:sqrt (22)]
	% [1 : ZZ : sqrt (22)]
	


	%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
	%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
	%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
	
	% Archive

	% clear all
	% figure
	% [1 : 3 : 20]
	 
	% a = 8;
	% b = 9;
	% c = a + b % results

	
	% A = 1:9
	
	% s = 2 * 3
	
	% clear a b c
	
	% 
	% b = ~1;
	% c = sin (3);
	

	% a = 7 + 8 + 9;
	
	% b = [ ...
	     % 3  ...
		 % 4  ...
		 % 5];
		 
	% b = [3 ; 4; 5];
		 
	% strings
	% y = 'asdf';
	% y = 'as\'df';
	% s = sprintf ('%f', a);
	% 

	% transpose
	% z2 = c123';
	% z3 = dd3';	
	% a4 = [1, sqrt (2), -3]'   

	% decimal
	% z2 = .4;	
	% aa234 = 3.45 + 6.78;	
	% z = A.*b;
	% z4 = +.4;
	% z3 = 123.456;

	% unary
	% x = -7;
	% y = -c123;
	% z = c -7;
	% z = 5 -7;
	% a = c * -7;
	% b = 3 * -8 + -9 * +77
	% a = b -7;
	% x = -c123 (6);
	% a = -sin (3.14);
	% -c123 (11);
	% -321 + -c123 + -sin (22)
	% 12 + -77 + -sin (22)
	% 123 * -456
	% A & ~c123
	% A & ~a1
	% +321 + 18
	% ~321 + 18

	% sin(a);
	% [1: 3 : 22] ; % a comment
	
	% two-char operators
	% z2 = a ~= b;
	% z3 = A.*B;
	
	% exponential
	% p = 123e45;
	% p = -23e-0.5;
	% p = +24e+6.8;
	
	% a3 = (3^r) + 7;
	% y1 = a1 + b12;
	
	% z = [1, 2, 3];
	% x = [1 ; 2 ; 3];
	% c = [1 : 2];
	% v = [1  2  3];
	% y1 = sin (a1);
	% c = c123 (5);
	