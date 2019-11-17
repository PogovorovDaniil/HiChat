import socket
import threading

def Client(id, conn, addr):
	try:
		data = bytearray(1)
		data[0] = id
		conn.send(data)
		print('connected:', addr, id)
		while 1:
			data = bytearray(conn.recv(256))
			To = data[0]
			data[0] = id
			SendTo(To, data)
	except Exception as ex:
		print('Опа! Произошла ошибка - ', ex)
		conn.close()
		return
	conn.close()
def SendTo(id, message):
	try:
		clients[id][0].send(message)
		print('message send:', id, message)
	except Exception as ex:
		print('Опа! Произошла ошибка - ', ex)

sock = socket.socket()
try:
	#sock.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEADDR, 1)
	sock.bind(('', 9090))
	sock.listen(0)

	id = 0
	clients = []

	while 1:
		clients.append(sock.accept())
		potok = threading.Thread(target= Client, args=(id, clients[id][0], clients[id][1]))
		potok.start()
		id = id + 1
		if id > 255:
			id = 0
			clients = []
except Exception as ex:
	print('Опа! Произошла ошибка - ', ex)
	sock.close()