sudo rabbitmqctl add_vhost dot-net-course
sudo rabbitmqctl set_permissions -p dot-net-course admin ".*" ".*" ".*"

sudo rabbitmqadmin -V dot-net-course --username=admin --password=password declare exchange name=Module2.Sample4.Exchange type=direct durable=true

sudo rabbitmqadmin -V dot-net-course --username=admin --password=password declare queue name=Module2.Sample4.Queue1 durable=true
sudo rabbitmqadmin -V dot-net-course --username=admin --password=password declare queue name=Module2.Sample4.Queue2 durable=true

sudo rabbitmqadmin -V dot-net-course --username=admin --password=password declare binding source=Module2.Sample4.Exchange destination=Module2.Sample4.Queue1 routing_key="1"
sudo rabbitmqadmin -V dot-net-course --username=admin --password=password declare binding source=Module2.Sample4.Exchange destination=Module2.Sample4.Queue2 routing_key="2"