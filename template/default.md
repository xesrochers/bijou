<!DOCTYPE html>
<html>
	<head>
        <meta charset="utf-8">
        <meta http-equiv="X-UA-Compatible" content="IE=edge">
        <meta name="description" content="Bijou Static HTML Website Generator">
        <meta name="viewport" content="width=device-width, initial-scale=1">

		<title>Bijou - {$title}</title>
		<link href='http://fonts.googleapis.com/css?family=Orbitron' rel='stylesheet' type='text/css' />		
        <link href='{$root}/styles/font-awesome.css' rel='stylesheet' />		
		<link href='{$root}/styles/superfish.css' rel='stylesheet' />
        <link href='{$root}/styles/style.css' rel='stylesheet' />		
	</head>
	<body>
        <!--[if lt IE 7]> 
            <p class="browsehappy">You are using an <strong>outdated</strong> browser. Please <a href="http://browsehappy.com/">upgrade your browser</a> to improve your experience.</p>
        <![endif]-->

		<div id='wrapper'>
			<header>
				<h1><a href='{$root}'>Bijou</a></h1>
			</header>	
			<nav id='topnav' class='inline'>
				{$topnav}
			</nav>
			<div id='content'>
				{$content}
			</div>
			<nav id='bottomnav'>
				{$topnav}
			</nav>
			<footer>
				<p>Powered by <a href='https://github.com/xesroche/bijou' target='_blank'>Bijou</a></p>
			</footer>
		</div>
		<script src="{$root}/scripts/jquery.min.js"></script>
		<script src="{$root}/scripts/hoverIntent.js"></script>
		<script src="{$root}/scripts/superfish.js"></script>
		<script src="{$root}/scripts/jquery.fastlivefilter.js"></script>
		<script src='{$root}/scripts/jquery.ready.js'></script>
	</body>
</html>
