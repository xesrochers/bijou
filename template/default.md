<html>
	<head>
        <link href='/styles/font-awesome.css' rel='stylesheet' />		
		<link href='/styles/superfish.css' rel='stylesheet' />
        <link href='/styles/style.css' rel='stylesheet' />		
	</head>
	<body>
		<div id='wrapper'>
			<header>
				<a href='/'><img src='/images/logo.svg' alt='logo' /></a>
			</header>	
			<nav id='topnav' class='inline'>
				{topnav}
			</nav>
			<nav id='breadcrumb'>
				{breadcrumb}
			</nav>
			<div id='content'>
				{content}
			</div>
			<footer>
				<p>Powered by bijou!</p>
			</footer>
		</div>
		<script src="/scripts/jquery-1.9.0.min.js"></script>
		<script src="/scripts/hoverIntent.js"></script>
		<script src="/scripts/superfish.js"></script>
		<script src='/scripts/jquery.ready.js' type='text/javascript' ></script>
	</body>
</html>