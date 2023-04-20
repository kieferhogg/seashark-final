import React, { useState, useEffect } from "react";
import * as signalR from "@microsoft/signalr";
import { Product, Order } from "../models/types";
import * as orderService from "../api/orderApi"
import * as productService from "../api/productApi"

function App() {
  const [products, setProducts] = useState<Product[]>([]);
  const [orders, setOrders] = useState<Order[]>([]);
  const [connection, setConnection] = useState<signalR.HubConnection>();

  useEffect(() => {
    const newConnection = new signalR.HubConnectionBuilder()
      .withUrl("/updatesHub")
      .build();

    setConnection(newConnection);
  }, []);

  useEffect(() => {
    if (connection) {
      connection.start().then(() => {
        console.log("connected");
      });
    }
  }, [connection]);

  useEffect(() => {
    if (connection) {
      connection.on("ReceiveProducts", (products: Product[]) => {
        setProducts(products);
      });
      connection.on("ReceiveOrders", (orders: Order[]) => {
        setOrders(orders);
      });
    }
  }, [connection]);

  useEffect(() => {
    productService.getProducts().then((products) => {
      setProducts(products);
    });
  }, []);

  useEffect(() => {
    orderService.getOrders().then((orders) => {
      setOrders(orders);
    });
  }, []);
  return (
    <div>
      <h1>Products</h1>
      <ul>
        {products.map((product) => (
          <li key={product.id}>
            {product.name} - {product.description}
            <button
              onClick={() => {
                productService
                  .updateProduct({ id: product.id, name: product.name, description: product.description })
                  .then((product) => {
                    setProducts(
                      products.map((p) => (p.id === product.id ? product : p))
                    );
                  });
              }}
            >
              Update
            </button>
            <button
              onClick={() => {
                if (product.id === undefined) return;
                productService.deleteProduct(product.id).then(() => {
                  setProducts(products.filter((p) => p.id !== product.id));
                });
              }}
            >
              Delete
            </button>
          </li>
        ))}
      </ul>
      <h2>Create Product</h2>
      <form
        onSubmit={(e) => {
          e.preventDefault();
          const target = e.target as typeof e.target & {
            name: { value: string };
            description: { value: string };
          };
          const name = target.name.value;
          const description = target.description.value;
          productService.createProduct({ name, description }).then((product) => {
            setProducts([...products, product]);
          });
        }}
      >
        <input type="number" name="name" />
        <input type="number" name="description" />
        <button type="submit">Create</button>
      </form>

      <h1>Orders</h1>
      <ul>
        {orders.map((order) => (
          <li key={order.id}>
            {order.productID} - {order.quantity}
            <button
              onClick={() => {
                orderService
                  .updateOrder({ id: order.id, productID: order.productID, quantity: order.quantity })
                  .then((order) => {
                    setOrders(
                      orders.map((o) => (o.id === order.id ? order : o))
                    );
                  });
              }}
            >
              Update
            </button>
            <button
              onClick={() => {
                if (order.id === undefined) return;
                orderService.deleteOrder(order.id).then(() => {
                  setOrders(orders.filter((o) => o.id !== order.id));
                });
              }}
            >
              Delete
            </button>

          </li>
        ))}
      </ul>
      <h2>Create Order</h2>
      <form
        onSubmit={(e) => {
          e.preventDefault();
          const target = e.target as typeof e.target & {
            productID: { value: number };
            quantity: { value: number };
          };
          const productID = target.productID.value;
          const quantity = target.quantity.value;
          orderService.createOrder({ productID, quantity }).then((order) => {
            setOrders([...orders, order]);
          });
        }}
      >
        <input type="number" name="productID" />
        <input type="number" name="quantity" />
        <button type="submit">Create</button>
      </form>
    </div>
  );
}

export default App