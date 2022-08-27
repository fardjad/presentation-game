import { Mutex } from "async-mutex";
import { EventEmitter } from "events";
import zmq from "zeromq-ng";

const zmqDealer = new zmq.Dealer();
// tslint:disable-next-line:no-object-mutation
zmqDealer.receiveTimeout = 1000;
const receiverMutex = new Mutex();
const senderMutex = new Mutex();
const zmqSender = new EventEmitter();

/**
 * ZMQ Message receiver
 */
export const zmqReceiver = new EventEmitter();

// tslint:disable-next-line:no-let
let listening = false;

/**
 * Gets server status.
 *
 * @return {boolean} A boolean that indicates whether the server is listening or not.
 */
export const isListening = (): boolean => listening;

const startZmqReceiverLoop = async () => {
  while (!zmqDealer.closed && listening) {
    const release = await receiverMutex.acquire();
    try {
      const message = await zmqDealer.receive();
      zmqReceiver.emit("message", message.toString());
    } catch (ex) {
      // NOTHING
    } finally {
      release();
    }
  }
};

/**
 * Starts listening on specified host and port.
 *
 * @param {string} protocol - transport protocol
 * @param {string} host - host or interface address to listen on
 * @param {number} number - port to listen on
 * @return {Promise<void>} A promise that gets resolved after the socket is bound.
 */
export const listen = async (
  protocol: "inproc" | "ipc" | "pgm" | "tcp" | "ticp" | "vmci",
  host: string,
  port: number
): Promise<void> => {
  if (listening) {
    throw new Error("Socket is already in listening state!");
  }

  listening = true;
  await zmqDealer.bind(`${protocol}://${host}:${port}`);

  zmqSender.on("message", async message => {
    // to prevent multiple messages from being sent simultaneously
    const release = await senderMutex.acquire();

    if (!zmqDealer.closed) {
      await zmqDealer.send(message);
    }

    release();
  });

  startZmqReceiverLoop();
};

/**
 * Closes the socket.
 *
 * @return {Promise<void>} A promise that gets resolved once the socket is closed.
 */
export const close = async (): Promise<void> => {
  const release = await receiverMutex.acquire();
  listening = false;
  if (!zmqDealer.closed) {
    zmqDealer.close();
  }
  release();
  zmqSender.removeAllListeners();
};

/**
 * Sends a message through the opened socket.
 *
 * @param {string} message - Message to send.
 */
export const send = (message: string) => {
  zmqSender.emit("message", message);
};

export default {
  close,
  isListening,
  listen,
  send,
  zmqReceiver
};
