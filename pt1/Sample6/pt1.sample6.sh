sudo rabbitmqctl add_vhost dot-net-course
sudo rabbitmqctl set_permissions -p dot-net-course admin ".*" ".*" ".*"

sudo rabbitmqadmin -V dot-net-course --username=admin --password=password declare exchange name=Module2.Sample5.Exchange type=topic durable=true

sudo rabbitmqadmin -V dot-net-course --username=admin --password=password declare queue name=Module2.Sample5.Queue1 durable=true
sudo rabbitmqadmin -V dot-net-course --username=admin --password=password declare queue name=Module2.Sample5.Queue2 durable=true
sudo rabbitmqadmin -V dot-net-course --username=admin --password=password declare queue name=Module2.Sample5.Queue3 durable=true

sudo rabbitmqadmin -V dot-net-course --username=admin --password=password declare binding source=Module2.Sample5.Exchange destination=Module2.Sample5.Queue1 routing_key="*.high.*"
sudo rabbitmqadmin -V dot-net-course --username=admin --password=password declare binding source=Module2.Sample5.Exchange destination=Module2.Sample5.Queue2 routing_key="*.*.cupboard"
sudo rabbitmqadmin -V dot-net-course --username=admin --password=password declare binding source=Module2.Sample5.Exchange destination=Module2.Sample5.Queue3 routing_key="*.medium.*"
sudo rabbitmqadmin -V dot-net-course --username=admin --password=password declare binding source=Module2.Sample5.Exchange destination=Module2.Sample5.Queue3 routing_key="corporate.#"
