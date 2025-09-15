sudo rabbitmqctl add_vhost dot-net-course
sudo rabbitmqctl set_permissions -p dot-net-course admin ".*" ".*" ".*"

sudo rabbitmqadmin -V dot-net-course --username=admin --password=password declare exchange name=Module2.Sample3.Exchange type=fanout durable=true

sudo rabbitmqadmin -V dot-net-course --username=admin --password=password declare queue name=Module2.Sample3.Queue1 durable=true
sudo rabbitmqadmin -V dot-net-course --username=admin --password=password declare queue name=Module2.Sample3.Queue2 durable=true

sudo rabbitmqadmin -V dot-net-course --username=admin --password=password declare binding source=Module2.Sample3.Exchange destination=Module2.Sample3.Queue1 routing_key=""
sudo rabbitmqadmin -V dot-net-course --username=admin --password=password declare binding source=Module2.Sample3.Exchange destination=Module2.Sample3.Queue2 routing_key=""