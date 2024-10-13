import { onMounted, onUnmounted } from 'vue';
import { useRoomStore } from "@/stores/RoomStore";
import { useSignalRStore } from "@/stores/SignalRStore";
import type {Game, MiniGame, MiniGameRound, PlayerJoinedMsg, Room} from "@/Models/RoomModels";
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

    signalRStore.subscribe('CurrentGameUpdated', (game: Game) => {
        console.debug('Current game updated', game);
        roomStore.updateCurrentGame(game);
    });

    signalRStore.subscribe('CurrentMiniGameUpdated', (miniGame: MiniGame) => {
        console.debug('Current mini-game updated', miniGame);
        roomStore.updateCurrentMiniGame(miniGame);
    });

    signalRStore.subscribe('CurrentRoundUpdated', (round: MiniGameRound) => {
        console.debug('Current round updated', round);
        roomStore.updateCurrentRound(round);
    });
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
