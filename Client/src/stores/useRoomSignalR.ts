import { onMounted, onUnmounted } from 'vue';
import { useRoomStore } from "@/stores/RoomStore";
import { useSignalRStore } from "@/stores/SignalRStore";
import type { PlayerJoinedMsg, Room } from "@/Models/RoomModels";

export function useRoomSignalR() {
  const roomStore = useRoomStore();
  const signalRStore = useSignalRStore();

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
    await signalRStore.connect();
    setupSignalRSubscriptions();
  });

  onUnmounted(async () => {
    await signalRStore.disconnect();
  });

  return {
    room: roomStore.room,
  };
}
