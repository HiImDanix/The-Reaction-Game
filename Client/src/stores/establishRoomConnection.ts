import { onMounted, onUnmounted } from 'vue';
import { useRoomStore } from "@/stores/RoomStore";
import { useSignalRStore } from "@/stores/SignalRStore";
import type { PlayerJoinedMsg, Room } from "@/Models/RoomModels";
import {useUserStore} from "@/stores/UserStore";
import {Api} from "@/Api/Api";

export function establishRoomConnection() {
  const roomStore = useRoomStore();
  const signalRStore = useSignalRStore();
  const userStore = useUserStore();

  const setupSignalRSubscriptions = () => {
    signalRStore.subscribe('PlayerJoined', (dto: PlayerJoinedMsg) => {
      console.debug('Player joined', dto);
      roomStore.addPlayer(dto.player);
    });

    // signalRStore.subscribe('RoomUpdated', (updatedRoom: Room) => {
    //   console.debug('Room updated', updatedRoom);
    //   roomStore.setRoom(updatedRoom);
    // });
  };

  onMounted(async () => {
    const token = await userStore.getToken();
    if (!token) {
      throw new Error('No token found. Cannot establish SignalR connection.');
    }
    await signalRStore.connect(token);
    setupSignalRSubscriptions();
    roomStore.setRoom(await Api.getRoom());
  });
}
