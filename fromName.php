<?php 
$host = 'localhost'; // адрес сервера 
$database = '********'; // имя базы данных
$user = 'root'; // имя пользователя
$password = '********'; // пароль

$link = mysqli_connect($host, $user, $password, $database) 
or die("ERROR");

$query = 'SELECT * FROM users WHERE name ="' . addslashes($_GET["name"]) . '"';

$result = mysqli_query($link, $query) or die("ERROR"); 

while ($row = mysqli_fetch_array($result)) 
{
	print($row['name'] . ";" . $row['id'] . ";" . $row['pubkey']);
}
mysqli_close($link);
?>