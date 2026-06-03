

% TokenTests.m, for utTokens

	% AnnotatedString - test cases should be clean single statements

	% d = [1 ; 2 ; 3]
	% e = 22 / 7
 	% z = 3 + 4 * sin (45) * 5;
	% a = b' + c' + d' + e';
	% d = 'zxcv';
	% z = 'aa\'b';
	% a = sprintf ('%d', qwe);
		
	% AnnotatedStringSet - 
	% a = 4; b = a ^ 2; c = b ^ 2; d = c / 3
	% a = 4; b = a ^ 2; c = b ^ 2; d = c / 3;
	
	
	% TokenParsing
	% z = -987
	% x = -a;
	% c = ~c123 & 1;
	% d = -3 * -sin (4);
	t1 = [1 2 3]' + (4 5 6)' + [7:9]' + dd' + c123;
	b6 = (1, 2, 3, 4)';
	a4 = [1, sqrt (2), -3]'
	zz = 'asd' + 'fg';

	% TokenUtils
	% [2 ; sin (z) ; 6^2]
	% [Abc, 456, 789]
 	% [1,2,3]
	% [11, 12 , 13]
	% [4 5 6]
	% ['aaa', 'sss']
	% [1 : 3 : 20]
	


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
	