import asyncio
from websockets import ServerConnection
from websockets.asyncio.server import serve
import cv2
import numpy as np

async def handle_client(websocket: ServerConnection):
  await websocket.send("Welcome to the WebSocket server!")
  try:
    width = 640
    height = 480

    async for message in websocket:
      if(isinstance(message, bytes)):

        np_array = np.frombuffer(message, dtype=np.uint8)
        image = cv2.imdecode(np_array, cv2.IMREAD_COLOR)
        if image is not None:
          image = cv2.flip(image, 1)
          # _ , image = cv2.threshold(image, 127, 255, cv2.THRESH_BINARY)

          _, data = cv2.imencode(".png", image)

          await websocket.send(data.tobytes())


      elif isinstance(message, str):
        width, height = map(int, message.split(","))

  except Exception as e:
    print(f"Error: {e}")
  finally:
    await websocket.close()

async def main():
  async with serve(handle_client, "localhost", 3000) as server:
    print("Server started on ws://localhost:3000")
    await server.wait_closed()

if __name__ == "__main__":
  try:
    asyncio.run(main())
  except KeyboardInterrupt:
    print("\nServer stopped!")