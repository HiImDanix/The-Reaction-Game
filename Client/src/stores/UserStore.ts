import {defineStore} from "pinia";
import {ref} from "vue";
import type {Player} from "@/Models/RoomModels";

interface UserState {
    user: Player | null;
    token: string | null;
}

const USER_STATE_STORAGE_KEY = 'userState';

export const useUserStore = defineStore('user', () => {
    const state = ref<UserState>(loadState());

    function loadState(): UserState {
        const savedState = localStorage.getItem(USER_STATE_STORAGE_KEY);
        return savedState ? JSON.parse(savedState) : { user: null, token: null };
    }

    function setUser(user: Player, token: string): void {
        state.value = { user: user, token: token };
        saveState();
    }

    function logout(): void {
        state.value = { user: null, token: null };
        saveState();
    }

    function saveState(): void {
        localStorage.setItem(USER_STATE_STORAGE_KEY, JSON.stringify(state.value));
    }

    function getToken(): string | null {
        const savedState = localStorage.getItem(USER_STATE_STORAGE_KEY);
        return savedState ? JSON.parse(savedState).token : null;
    }

    return {
        get user() { return state.value.user },
        getToken,
        setUser,
        logout
    }
});
