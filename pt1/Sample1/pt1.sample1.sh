sudo rabbitmqctl add_vhost dot-net-course
sudo rabbitmqctl set_permissions -p dot-net-course admin ".*" ".*" ".*"
sudo rabbitmqadmin -V dot-net-course --username=admin --password=password declare queue name=Module2.Sample1 durable=true