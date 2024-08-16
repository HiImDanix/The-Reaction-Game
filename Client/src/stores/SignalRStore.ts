import {defineStore} from "pinia";
import {ref} from "vue";
import {HubConnection, HubConnectionBuilder} from "@microsoft/signalr";
import {useUserStore} from "@/stores/UserStore";

export enum ConnectionState {
    Disconnected,
    Connecting,
    Connected,
    Reconnecting,
}

export const useSignalRStore = defineStore('signalR', () => {
    const connection = ref<HubConnection | null>(null);
    const connectionState = ref<ConnectionState>(ConnectionState.Disconnected);
    const subscriptions = ref<{ message: string; callback: (data: any) => void }[]>([]);

    async function connect(): Promise<void> {
        if (connectionState.value === ConnectionState.Connected) {
            console.debug('SignalR connection already established.');
            return;
        }
        if (connectionState.value === ConnectionState.Connecting) {
            console.debug('SignalR connection is already in progress.');
            return;
        }
        connectionState.value = ConnectionState.Connecting;

        const maxRetries = 3;
        let retryCount = 0;
        let retryDelay = 1000;
        const token = (await useUserStore()).getToken();
        console.log('Token:', token);
        while (retryCount < maxRetries) {
            try {
                connection.value = new HubConnectionBuilder()
                    .withUrl('https://localhost:7211/lobbyHub', {
                        accessTokenFactory: () => token
                    })
                    .withAutomaticReconnect()
                    .build();

                await connection.value.start();
                connectionState.value = ConnectionState.Connected;
                console.debug('SignalR connection established.');

                // Subscribe to all previously subscribed messages
                subscriptions.value.forEach(subscription => {
                    console.debug('Subscribing to message:', subscription.message);
                    connection.value.on(subscription.message, subscription.callback);
                });

                connection.value.onreconnecting(() => {
                    connectionState.value = ConnectionState.Reconnecting;
                    console.debug('SignalR connection lost. Reconnecting...');
                });

                connection.value.onreconnected(() => {
                    connectionState.value = ConnectionState.Connected;
                    console.debug('SignalR connection re-established.');
                });

                connection.value.onclose(() => {
                    connectionState.value = ConnectionState.Disconnected;
                    console.debug('SignalR connection closed.');
                });

                return;
            } catch (error) {
                console.error('SignalR connection failed.', error);
                retryCount++;
                retryDelay *= 2;
                await new Promise(resolve => setTimeout(resolve, retryDelay));
            }
        }

        connectionState.value = ConnectionState.Disconnected;
        console.error('SignalR connection failed after max retries.');
    }

    async function disconnect(): Promise<void> {
        if (connectionState.value === ConnectionState.Connected) {
            await connection.value.stop();
            connection.value = null;
            connectionState.value = ConnectionState.Disconnected;
        }
    }

    function subscribe<T>(message: string, callback: (data: T) => void): void {
        if (connectionState.value === ConnectionState.Connected && !subscriptions.value.some(sub => sub.message === message)) {
            connection.value.on(message, callback);
        }
        subscriptions.value.push({message, callback});

    }

    return {connection, connectionState, connect, disconnect, subscribe};

})
